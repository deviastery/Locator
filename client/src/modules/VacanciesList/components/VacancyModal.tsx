import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import Divider from '@mui/material/Divider';
import Alert from '@mui/material/Alert';
import { Modal } from '../../../components/Modal.tsx';
import type { VacancyResponse } from "../types/responses/VacancyResponse.ts";
import { useEffect, useState } from "react";
import { fetchVacancyById } from "../api/fetchVacancyById.ts";
import { createNegotiation } from "../api/createNegotiation.ts";
import { getRatingColor } from "../utils/vacancyRatingStyles.ts";
import { ReviewsList } from './ReviewsList';
import { RespondButton } from '../ui/RespondButton';

interface VacancyModalProps {
    open: boolean;
    onClose: () => void;
    vacancyId: number;
}

export const VacancyModal = ({ open, onClose, vacancyId }: VacancyModalProps) => {
    const [vacancyData, setVacancyData] = useState<VacancyResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [responding, setResponding] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    useEffect(() => {
        if (!open) return;

        const loadVacancy = async () => {
            setLoading(true);
            const data = await fetchVacancyById(vacancyId);
            setVacancyData(data);
            setLoading(false);
        };

        loadVacancy();
    }, [vacancyId, open]);

    const handleRespond = async () => {
        setResponding(true);
        setError(null);

        const result = await createNegotiation(vacancyId);

        setResponding(false);

        if (result.success) {
            setSuccess(true);
            setTimeout(() => {
                setSuccess(false);
                onClose();
            }, 1500);
        } else {
            setError(result.error || 'Не удалось отправить отклик');
        }
    };

    if (!open) return null;

    if (loading || !vacancyData) {
        return (
            <Modal open={open} onClose={onClose}>
                <Typography>Загрузка...</Typography>
            </Modal>
        );
    }

    const vacancy = vacancyData.vacancy;
    const reviews = vacancyData.reviews || [];
    const ratingColor = getRatingColor(vacancy.rating);

    return (
        <Modal open={open} onClose={onClose}>
            <Box
                sx={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'flex-start',
                    mb: 1,
                }}
            >
                <Box sx={{ flex: 1 }}>
                    <Typography variant="h4" sx={{ fontWeight: 700, mb: 1 }}>
                        {vacancy.name}
                    </Typography>
                    <Typography variant="body1" color="text.secondary">
                        Опыт: {vacancy.experience?.name || 'Не указан'}
                    </Typography>
                </Box>
                <Box sx={{ textAlign: 'right' }}>
                    <Typography variant="h5" sx={{ fontWeight: 700, mb: 1 }}>
                        {vacancy.salary?.from || vacancy.salary?.to
                            ? `${vacancy.salary.from || ''} ${vacancy.salary.from && vacancy.salary.to ? '–' : ''} ${vacancy.salary.to || ''} ${vacancy.salary.currency}`
                            : 'Не указана'
                        }
                    </Typography>
                    {vacancy.rating && (
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                            <Typography sx={{ whiteSpace: 'nowrap', color: ratingColor }}>
                                Рейтинг вакансии {vacancy.rating.toFixed(1)}
                            </Typography>
                        </Box>
                    )}
                </Box>
            </Box>

            <Box display="flex" gap={2} mb={1}>
                <Typography variant="body1">
                    {vacancy.employer || 'Не указана'}
                </Typography>
                <Typography variant="body1">
                    {vacancy.area?.name || 'Не указан'}
                </Typography>
            </Box>

            <Divider sx={{ mb: 3 }} />

            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}

            {success && (
                <Alert severity="success" sx={{ mb: 2 }}>
                    Отклик успешно отправлен!
                </Alert>
            )}

            <Box sx={{ mb: 3 }}>
                <Typography variant="body2" color="text.secondary">
                    {vacancy.responsibility || 'Описание отсутствует'}
                </Typography>
            </Box>

            <Divider sx={{ mb: 2 }} />

            <Box sx={{ mb: 3 }}>
                <Typography variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
                    Отзывы
                </Typography>
                <ReviewsList reviews={reviews} />
            </Box>

            <Divider sx={{ mb: 3 }} />

            <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                <Link
                    href={vacancy.url}
                    target="_blank"
                    rel="noopener noreferrer"
                    sx={{
                        color: '#0f0c5f',
                        textTransform: 'none',
                        fontSize: '1rem',
                        fontWeight: 600,
                        textDecoration: 'none',
                        '&:hover': {
                            textDecoration: 'underline',
                        },
                    }}
                >
                    Перейти на страницу вакансии
                </Link>
                <RespondButton
                    onClick={handleRespond}
                    loading={responding}
                    success={success}
                />
            </Box>
        </Modal>
    );
};
