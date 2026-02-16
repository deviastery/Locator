import type { Negotiation } from '../Negotiation';

export interface NegotiationsResponse {
    negotiations: Negotiation[];
    pages: number;
}

export interface UnauthorizedResponse {
    unauthorized: true;
}

export type NegotiationsFetchResult = NegotiationsResponse | UnauthorizedResponse | null;
