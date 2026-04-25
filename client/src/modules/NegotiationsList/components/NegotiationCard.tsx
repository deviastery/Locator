import Paper from '@mui/material/Paper';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import type { Negotiation } from '../types/Negotiation';
import { getDaysAgo, getDaysText } from '../utils/parseDate';
import { useState } from "react";
import { ReviewModal } from './ReviewModal';
import { getCardStyle } from "../utils/negotiationCardStyles.ts";
import { CreateReviewButton } from "../ui/CreateReviewButton";

interface NegotiationCardProps {
    negotiation: Negotiation;
}

export const NegotiationCard = ({ negotiation }: NegotiationCardProps) => {
    const { vacancy, state, viewed_by_opponent, created_at } = negotiation;
    const cardStyle = getCardStyle(state.id, viewed_by_opponent);
    const [modalOpen, setModalOpen] = useState(false);

    const daysAgo = getDaysAgo(created_at);
    const canLeaveReview = daysAgo >= 5;
    return (
        <>
            <Paper
                elevation={0}
                sx={{
                    mx: 'auto',
                    mb: 2,
                    maxWidth: 1200,
                    borderRadius: 3,
                    bgcolor: cardStyle.bgcolor,
                    px: 3,
                    py: 2,
                }}
            >
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
                    <Box flex={1}>
                        <Typography variant="h6" fontWeight={700}>
                            {vacancy.name}
                        </Typography>
                        <Box display="flex" gap={2}>
                            <Typography variant="body2" color="text.secondary">
                                {vacancy.employer.name || 'Не указана'}
                            </Typography>
                            <Typography variant="body2" color="text.secondary">
                                {vacancy.area?.name || 'Не указан'}
                            </Typography>
                        </Box>
                    </Box>

                    <CreateReviewButton
                        canLeaveReview={canLeaveReview}
                        onClick={() => setModalOpen(true)}
                    />
                </Box>

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', gap: 2 }}>
                    <Typography flex={1} color="text.primary">
                        {vacancy.snippet.responsibility || 'Описание отсутствует'}
                    </Typography>

                    <Box sx={{ textAlign: 'right', whiteSpace: 'nowrap' }}>
                        <Typography variant="body2" color="text.secondary">
                            {getDaysText(getDaysAgo(created_at))}
                        </Typography>
                        <Typography variant="body2" sx={{ fontWeight: 600, color: cardStyle.statusColor }}>
                            {cardStyle.statusText}
                        </Typography>
                    </Box>
                </Box>
            </Paper>
            
            <ReviewModal
                open={modalOpen}
                onClose={() => setModalOpen(false)}
                vacancyTitle={vacancy.name}
                vacancyId={vacancy.id}
            />
        </>
    );
};