import type { State, Vacancy } from "./Vacancy";

export interface Negotiation {
    id: string;
    state: State;
    viewed_by_opponent: boolean;
    created_at: string;
    vacancy: Vacancy;
}
