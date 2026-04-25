export const handleUnauthorized = () => {
    localStorage.removeItem('vacanciesPage');
    localStorage.removeItem('negotiationsPage');

    window.location.href = '/auth';
};
