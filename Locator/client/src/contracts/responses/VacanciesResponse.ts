import type { FullVacancyDto } from '../dtos/FullVacancyDto';

export interface VacanciesResponse {
    vacancies: FullVacancyDto[];
    pages: number;
}

export interface UnauthorizedResponse {
    unauthorized: true;
}

export type VacanciesFetchResult = VacanciesResponse | UnauthorizedResponse | null;