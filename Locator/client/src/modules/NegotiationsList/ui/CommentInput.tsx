import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import TextField from '@mui/material/TextField';

interface CommentInputProps {
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
}

export const CommentInput = ({ value, onChange, placeholder }: CommentInputProps) => {
    return (
        <Box sx={{ mb: 3 }}>
            <Typography variant="body1" sx={{ mb: 1 }}>
                Напишите отзыв
            </Typography>
            <TextField
                multiline
                rows={6}
                fullWidth
                placeholder={placeholder}
                value={value}
                onChange={(e) => onChange(e.target.value)}
                sx={{
                    '& .MuiOutlinedInput-root': {
                        borderRadius: 2,
                    },
                }}
            />
        </Box>
    );
};
