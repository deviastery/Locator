import Box from '@mui/material/Box';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Typography from '@mui/material/Typography';

interface OnlyWithReviewsCheckboxProps {
    checked: boolean;
    onChange: (checked: boolean) => void;
}

export const OnlyWithReviewsCheckbox = ({ checked, onChange }: OnlyWithReviewsCheckboxProps) => {
    return (
        <Box sx={{ mb: 4 }}>
            <FormControlLabel
                control={
                    <Checkbox
                        checked={checked}
                        onChange={(e) => onChange(e.target.checked)}
                        sx={{
                            color: 'rgba(255, 255, 255, 0.7)',
                            '&.Mui-checked': {
                                color: 'white',
                            },
                        }}
                    />
                }
                label={
                    <Typography variant="h6" sx={{ fontWeight: 600 }}>
                        Только с отзывами
                    </Typography>
                }
            />
        </Box>
    );
};