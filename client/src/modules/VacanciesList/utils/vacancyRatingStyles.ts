export const getRatingColor = (rating: number | undefined) => {
    if (!rating) return 'text.secondary';
    if (rating >= 4.0) return 'success.main';
    if (rating >= 3.0) return 'warning.main';
    return 'error.main';
};