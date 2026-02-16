import { VacancyCard } from "./components/VacancyCard.tsx";
import type { FullVacancyDto } from '../../contracts/dtos/FullVacancyDto.ts';
import { useEffect, useState, useRef } from "react";
import { fetchVacancies } from "./api/fetchVacancies.ts";
import type { GetVacanciesDto } from "../../contracts/dtos/GetVacanciesDto.ts";
import { useSearchParams } from 'react-router-dom';
import { CenteredBox } from "../../components/CenteredBox";
import { ListWithPagination } from "../../components/ListWithPagination";

interface VacanciesListProps {
    searchQuery: string;
    filters: Partial<GetVacanciesDto>;
}

export const VacanciesList = ({ searchQuery, filters }: VacanciesListProps) => {
    const [searchParams, setSearchParams] = useSearchParams();
    const [vacancies, setVacancies] = useState<FullVacancyDto[] | null>(null);
    const [loading, setLoading] = useState(true);
    const [totalPages, setTotalPages] = useState(0);
    const [debouncedQuery, setDebouncedQuery] = useState(searchQuery);
    const isInitialMount = useRef(true);
    const perPage = 10;

    const page = parseInt(searchParams.get('page') || '1', 10);

    useEffect(() => {
        localStorage.setItem('vacanciesPage', page.toString());
    }, [page]);

    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedQuery(searchQuery);
            if (!isInitialMount.current && searchQuery !== debouncedQuery) {
                setSearchParams({ page: '1' }, { replace: true });
            }
        }, 3000);

        return () => clearTimeout(timer);
    }, [searchQuery, debouncedQuery, setSearchParams]);

    useEffect(() => {
        const loadVacancies = async () => {
            setLoading(true);

            const response = await fetchVacancies({
                perPage: perPage,
                page: page,
                searchField: debouncedQuery || undefined,
                ...filters,
            });

            if (response && 'unauthorized' in response && response.unauthorized) {
                setVacancies([]);
            } else if (response && 'vacancies' in response) {
                setVacancies(response.vacancies);
                setTotalPages(response.pages);
            } else {
                setVacancies([]);
            }
            setLoading(false);

            if (isInitialMount.current) {
                isInitialMount.current = false;
            }
        };

        loadVacancies();
    }, [page, debouncedQuery, filters]);

    const handlePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
        setSearchParams({ page: value.toString() }, { replace: true });
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    if (loading || vacancies === null) {
        return <CenteredBox>Загрузка...</CenteredBox>;
    }
    if (vacancies.length === 0) {
        return <CenteredBox>Не найдено подходящих вакансий</CenteredBox>;
    }

    return (
        <ListWithPagination
            totalPages={totalPages}
            page={page}
            onChange={handlePageChange}
        >
            {vacancies.map(v => (
                <VacancyCard key={v.id} vacancy={v} />
            ))}
        </ListWithPagination>
    );
};
