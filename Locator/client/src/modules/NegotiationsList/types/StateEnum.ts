export const StateEnum = {
    WAITING: 'response' as const,
    ACCEPTED: 'invitation' as const,
    REJECTED: 'discard' as const,
};

export type StateEnum = typeof StateEnum[keyof typeof StateEnum];