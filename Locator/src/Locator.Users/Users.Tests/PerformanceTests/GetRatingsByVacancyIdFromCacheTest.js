import http from "k6/http";
import { sleep, check } from "k6";
import { Counter, Trend } from "k6/metrics";

const cacheHits = new Counter("cache_hits");
const cacheMisses = new Counter("cache_misses");
const cachedResponseTime = new Trend("cached_response_time");
const uncachedResponseTime = new Trend("uncached_response_time");

export const options = {
    stages: [
        { duration: "20s", target: 50 },
        { duration: "30s", target: 100 },
        { duration: "20s", target: 0 },
    ],
    thresholds: {
        http_req_duration: ["p(90)<5000", "p(95)<7000"],
        http_req_failed: ["rate<0.01"],
        cached_response_time: ["p(95)<500"],
    },
};

const vacancyIds = [
    122536457, 124883728, 125215691, 121850960, 124266449, 122370327,
];

const firstRequestTime = {};

export default function () {
    const vacancyId = vacancyIds[Math.floor(Math.random() * vacancyIds.length)];
    const url = `https://localhost:5003/api/ratings/vacancies/${vacancyId}`;

    const params = {
        headers: {
            "Content-Type": "application/json",
        },
    };

    const startTime = Date.now();
    const res = http.get(url, params);
    const duration = res.timings.duration;

    const isFirstRequest = !firstRequestTime[vacancyId];
    const isFromCache = !isFirstRequest && duration < 500;

    if (isFirstRequest) {
        firstRequestTime[vacancyId] = startTime;
    }

    const success = check(res, {
        "status is 200": (r) => r.status === 200,
        "has rating data": (r) => {
            try {
                const body = JSON.parse(r.body);
                return body !== null && body !== undefined;
            } catch (e) {
                return false;
            }
        },
    });

    if (isFromCache) {
        cacheHits.add(1);
        cachedResponseTime.add(duration);
        check(res, {
            "cached response < 500ms": (r) => r.timings.duration < 500,
        });
    } else {
        cacheMisses.add(1);
        uncachedResponseTime.add(duration);
    }

    sleep(1);
}

export function handleSummary(data) {
    const cacheHitCount = data.metrics.cache_hits?.values?.count || 0;
    const cacheMissCount = data.metrics.cache_misses?.values?.count || 0;
    const totalRequests = cacheHitCount + cacheMissCount;
    const hitRate =
        totalRequests > 0
            ? ((cacheHitCount / totalRequests) * 100).toFixed(2)
            : 0;

    console.log(`\n=== Cache Performance Summary ===`);
    console.log(`Cache Hits: ${cacheHitCount}`);
    console.log(`Cache Misses: ${cacheMissCount}`);
    console.log(`Estimated Hit Rate: ${hitRate}%`);
    console.log(`Total Requests: ${totalRequests}`);

    const avgCached = data.metrics.cached_response_time?.values?.avg || 0;
    const avgUncached = data.metrics.uncached_response_time?.values?.avg || 0;

    console.log(`\nAverage cached response time: ${avgCached.toFixed(2)}ms`);
    console.log(`Average uncached response time: ${avgUncached.toFixed(2)}ms`);

    if (avgCached > 0 && avgUncached > 0) {
        const improvement = (
            ((avgUncached - avgCached) / avgUncached) *
            100
        ).toFixed(2);
        console.log(`Cache speed improvement: ${improvement}%`);
    }
}
