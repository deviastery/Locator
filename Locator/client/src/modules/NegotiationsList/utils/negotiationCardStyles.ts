import { StateEnum } from '../types/StateEnum.ts';

export const getCardStyle = (state: StateEnum, viewedByOpponent: boolean) => {
    if (state === StateEnum.ACCEPTED) {
        return {
            bgcolor: '#c8e6c9',
            statusText: 'Приглашение',
            statusColor: '#2e7d32',
        };
    }

    if (state === StateEnum.REJECTED) {
        return {
            bgcolor: '#ffcdd2',
            statusText: 'Отказ',
            statusColor: '#c62828',
        };
    }

    if (viewedByOpponent) {
        return {
            bgcolor: '#a8b8ff',
            statusText: 'Просмотрено',
            statusColor: '#0f3bff',
        };
    }

    return {
        bgcolor: '#e1d5f5',
        statusText: 'Не просмотрено',
        statusColor: '#545454',
    };
};