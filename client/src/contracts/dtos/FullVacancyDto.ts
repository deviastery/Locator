export interface Area {
    id: string;
    name: string;
    url?: string;
}

export interface Address {
    city?: string;
    street?: string;
    building?: string;
    description?: string;
    lat?: number;
    lng?: number;
    raw?: string;
}

export interface Experience {
    id: string;
    name: string;
}

export interface Salary {
    from?: number;
    to?: number;
    currency: string;
    gross?: boolean;
}

export interface Schedule {
    id: string;
    name: string;
}

export interface WorkFormat {
    id: string;
    name: string;
}

export interface FullVacancyDto {
    id: number;
    name: string;
    url: string;
    responsibility?: string;
    employer?: string;
    area?: Area;
    address?: Address;
    experience?: Experience;
    salary?: Salary;
    schedule?: Schedule;
    workFormat?: WorkFormat[];
    rating?: number;
}