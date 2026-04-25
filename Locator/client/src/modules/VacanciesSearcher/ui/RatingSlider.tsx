import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Slider from '@mui/material/Slider';

interface RatingSliderProps {
    value: number[];
    onChange: (value: number[]) => void;
}

export const RatingSlider = ({ value, onChange }: RatingSliderProps) => {
    return (
        <Box sx={{ mb: 4 }}>
            <Typography variant="h6" sx={{ mb: 2, fontWeight: 600 }}>
                Рейтинг вакансии
            </Typography>
            <Box sx={{ px: 1 }}>
                <Slider
                    value={value}
                    onChange={(_e, newValue) => onChange(newValue as number[])}
                    valueLabelDisplay="auto"
                    min={0}
                    max={5}
                    step={0.5}
                    sx={{
                        color: 'white',
                        '& .MuiSlider-thumb': {
                            bgcolor: 'white',
                        },
                        '& .MuiSlider-track': {
                            bgcolor: 'white',
                        },
                        '& .MuiSlider-rail': {
                            bgcolor: 'rgba(255,255,255,0.3)',
                        },
                        '& .MuiSlider-valueLabel': {
                            color: 'black',
                            bgcolor: 'white',
                        }
                    }}
                />
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
                    <Typography variant="body2">От: {value[0]}</Typography>
                    <Typography variant="body2">До: {value[1]}</Typography>
                </Box>
            </Box>
        </Box>
    );
};