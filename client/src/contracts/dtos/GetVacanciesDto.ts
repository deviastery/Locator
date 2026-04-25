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
    minRating?: number;
    maxRating?: number;
    onlyWithReviews?: boolean;
    perPage?: number;
    pages?: number;
    page?: number;
}

