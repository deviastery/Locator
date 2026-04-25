import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import TextField from '@mui/material/TextField';

interface RatingInputProps {
    value: string;
    onChange: (value: string) => void;
}

export const RatingInput = ({ value, onChange }: RatingInputProps) => {
    return (
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
            <Typography variant="body1">
                Поставьте оценку вакансии от 0.0 до 5.0*
            </Typography>
            <TextField
                size="small"
                type="number"
                value={value}
                onChange={(e) => onChange(e.target.value)}
                inputProps={{
                    min: 0,
                    max: 5,
                    step: 0.1,
                }}
                sx={{
                    width: 100,
                    '& .MuiOutlinedInput-root': {
                        borderRadius: 2,
                    },
                }}
            />
        </Box>
    );
};
