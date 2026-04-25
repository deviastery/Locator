export const getCurrencyLabel = (currency: string | undefined) => {
    if (!currency) return '';

    const currencyMap: Record<string, string> = {
        'RUR': '₽',
        'RUB': '₽',
        'EUR': '€',
        'USD': '$',
        'KZT': '₸',
        'UAH': '₴',
        'BYR': 'Br',
    };

    return currencyMap[currency] || currency;
};