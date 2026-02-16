export const parseDate = (dateStr: string | undefined | null): Date => {
    if (!dateStr || typeof dateStr !== 'string') {
        console.warn('Invalid date string:', dateStr);
        return new Date();
    }

    try {
        const parts = dateStr.split(' ');
        if (parts.length !== 2) {
            console.warn('Date string missing time part:', dateStr);
            return new Date();
        }

        const [datePart, timePart] = parts;
        const dateParts = datePart.split('.');
        const timeParts = timePart.split(':');

        if (dateParts.length !== 3 || timeParts.length !== 3) {
            console.warn('Invalid date format:', dateStr);
            return new Date();
        }

        const [day, month, year] = dateParts.map(Number);
        const [hours, minutes, seconds] = timeParts.map(Number);

        const parsedDate = new Date(year, month - 1, day, hours, minutes, seconds);

        return parsedDate;
    } catch (e) {
        console.error('Error parsing date:', dateStr, e);
        return new Date();
    }
};

export const getDaysAgo = (dateStr: string | undefined | null): number => {
    if (!dateStr) {
        return 0;
    }

    const createdDate = parseDate(dateStr);
    const now = Date.now();
    const diff = now - createdDate.getTime();
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));

    return days;
};

export const getDaysText = (days: number): string => {
    if (days === 0) return 'Сегодня';
    if (days === 1) return '1 день назад';
    if (days < 5) return `${days} дня назад`;
    return `${days} дней назад`;
};
