import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import { useState } from 'react';
import { VacancyModal } from './VacancyModal';
import type {FullVacancyDto } from "../../../contracts/dtos/FullVacancyDto.ts";
import { getCurrencyLabel } from "../utils/getCurrencyLabel.ts";
import { getRatingColor } from "../utils/vacancyRatingStyles.ts";

interface VacancyCardProps {
    vacancy: FullVacancyDto;
}

export const VacancyCard = ({ vacancy }: VacancyCardProps) => {
    const [modalOpen, setModalOpen] = useState(false);

    const ratingColor = getRatingColor(vacancy.rating);
    const currencyLabel = getCurrencyLabel(vacancy.salary?.currency);
    return (
        <>
            <Paper
                elevation={0}
                sx={{
                    mx: 'auto',
                    mb: 2,
                    maxWidth: 1200,
                    borderRadius: 3,
                    bgcolor: '#f4e6fa',
                    px: 3,
                    py: 2,
                    cursor: 'pointer',
                    transition: 'background-color 0.3s ease, box-shadow 0.3s ease',
                    '&:hover': {
                        bgcolor: '#ebd5f5',
                        boxShadow: 3,
                    },
                }}
                onClick={() => setModalOpen(true)}
            >
                <Box display="flex" justifyContent="space-between" mb={1}>
                    <Box display="flex" gap={1} alignItems="baseline">
                        <Typography variant="h6" fontWeight={700}>
                            {vacancy.name}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Опыт: {vacancy.experience?.name || 'Не указан'}
                        </Typography>
                    </Box>
                    <Typography fontWeight={600} whiteSpace="nowrap">
                        {vacancy.salary?.from || vacancy.salary?.to
                            ? `${vacancy.salary.from?.toLocaleString() || 
                            ''} ${vacancy.salary.from && vacancy.salary.to ? '–' : ''} ${vacancy.salary.to?.toLocaleString() 
                                || ''} ${currencyLabel}`.trim()
                            : 'Не указана'
                        }
                    </Typography>
                </Box>
    
                <Box display="flex" gap={2} mb={1}>
                    <Typography variant="body2" color="text.secondary">
                        {vacancy.employer || 'Не указана'}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        {vacancy.area?.name || 'Не указан'}
                    </Typography>
                </Box>
    
                <Box display="flex" alignItems="center" gap={2}>
                    <Typography
                        sx={{
                            flex: 1,
                            display: '-webkit-box',
                            WebkitLineClamp: 3,
                            WebkitBoxOrient: 'vertical',
                            overflow: 'hidden',
                            textOverflow: 'ellipsis',
                        }}
                        color="text.primary"
                    >
                        {vacancy.responsibility || 'Описание отсутствует'}
                    </Typography>
                    <Typography sx={{ whiteSpace: 'nowrap', color: ratingColor }}>
                        {vacancy.rating
                            ? `Рейтинг вакансии ${vacancy.rating.toFixed(1)}`
                            : 'Рейтинг не определен'
                        }
                    </Typography>
                </Box>
            </Paper>

            <VacancyModal
                open={modalOpen}
                onClose={() => setModalOpen(false)}
                vacancyId={vacancy.id}
            />
        </>
    );
};