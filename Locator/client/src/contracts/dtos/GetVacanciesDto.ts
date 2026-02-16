export interface SalaryQuery {
    from?: number;
    to?: number;
    currency?: string;
}

export interface GetVacanciesDto {
    searchField?: string;
    experience?: string;
    employment?: string;
    schedule?: string;
    area?: string;
    salary?: SalaryQuery;
    perPage?: number;
    pages?: number;
    page?: number;
}

