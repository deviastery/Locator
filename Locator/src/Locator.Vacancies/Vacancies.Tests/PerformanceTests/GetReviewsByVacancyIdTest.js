import http from "k6/http";
import { sleep } from "k6";

export const options = {
    stages: [
        { duration: "20s", target: 50 },
        { duration: "30s", target: 100 },
        { duration: "20s", target: 0 },
    ],
    thresholds: {
        http_req_duration: ["p(90)<5000"],
        http_req_failed: ["rate<0.01"],
    },
};

const vacancyIds = [
    122536457, 124883728, 125215691, 121850960, 124266449, 122370327,
];

export default function () {
    const vacancyId = vacancyIds[Math.floor(Math.random() * vacancyIds.length)];

    const url = `https://localhost:5003/api/vacancies/${vacancyId}/reviews`;

    const params = {
        headers: {
            "Content-Type": "application/json",
        },
    };

    http.get(url, params);

    sleep(1);
}
