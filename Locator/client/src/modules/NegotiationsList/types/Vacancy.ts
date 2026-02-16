import type {StateEnum} from "./StateEnum.ts";

export interface State {
    id: StateEnum;
    name: string;
}

export interface Snippet {
    requirement?: string | null;
    responsibility?: string | null;
}

export interface Employer {
    id: string;
    name: string;
}

export interface Area {
    id: string;
    name: string;
}

export interface Address {
    city?: string | null;
    metro_stations: any[];
}

export interface Experience {
    id: string;
    name: string;
}

export interface Salary {
    currency?: string | null;
    from?: number | null;
    gross?: boolean | null;
    to?: number | null;
}

export interface Schedule {
    id: string;
    name: string;
    uid?: string | null;
}

export interface WorkFormat {
    id: string;
    name: string;
}

export interface Vacancy {
    id: string;
    name: string;
    alternate_url: string;
    snippet: Snippet;
    employer: Employer;
    area: Area;
    address: Address;
    experience: Experience;
    salary: Salary;
    schedule: Schedule;
    work_format: WorkFormat[];
}