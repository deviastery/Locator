import { useState } from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { Modal } from '../../../components/Modal';
import Divider from "@mui/material/Divider";
import { createReview } from "../api/createReview.ts";
import { Alert } from '@mui/material';
import { CommentInput } from '../ui/CommentInput';
import { RatingInput } from '../ui/RatingInput';
import { CancelButton } from '../ui/CancelButton';
import { SubmitButton } from '../ui/SubmitButton';

interface ReviewModalProps {
    open: boolean;
    onClose: () => void;
    vacancyTitle: string;
    vacancyId: string;
}

export const ReviewModal = ({ open, onClose, vacancyTitle, vacancyId }: ReviewModalProps) => {
    const [rating, setRating] = useState<string>('5.0');
    const [comment, setComment] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    const handleSubmit = async () => {
        const mark = parseFloat(rating);

        if (isNaN(mark) || mark < 0 || mark > 5) {
            setError('Оценка должна быть от 0.0 до 5.0');
            return;
        }

        setLoading(true);
        setError(null);

        const result = await createReview(vacancyId, {
            Mark: mark,
            Comment: comment.trim() || undefined,
        });

        setLoading(false);

        if (result.success) {
            setSuccess(true);
            setTimeout(() => {
                handleCancel();
            }, 1500);
        } else {
            setError(result.error || 'Не удалось отправить отзыв');
        }
    };

    const handleCancel = () => {
        setRating('5.0');
        setComment('');
        setError(null);
        setSuccess(false);
        onClose();
    };

    return (
        <Modal open={open} onClose={onClose} maxWidth={600}>
            <Box>
                <Typography variant="h5" sx={{ fontWeight: 700, mb: 1 }}>
                    Оставьте отзыв по вакансии
                </Typography>

                <Divider sx={{ mb: 2 }} />

                <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
                    {vacancyTitle}
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                        {error}
                    </Alert>
                )}

                {success && (
                    <Alert severity="success" sx={{ mb: 2 }}>
                        Отзыв успешно отправлен!
                    </Alert>
                )}

                <RatingInput value={rating} onChange={setRating} />

                <CommentInput
                    value={comment}
                    onChange={setComment}
                    placeholder="Здесь вы можете написать, например, как быстро Вам ответили на отклик или как прошло собеседование"
                />

                <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
                    <CancelButton onClick={handleCancel} disabled={loading} />
                    <SubmitButton onClick={handleSubmit} loading={loading} success={success} />
                </Box>
            </Box>
        </Modal>
    );
};
