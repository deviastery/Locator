import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Slider from '@mui/material/Slider';

interface SalarySliderProps {
    value: number[];
    onChange: (value: number[]) => void;
}

export const SalarySlider = ({ value, onChange }: SalarySliderProps) => {
    return (
        <Box sx={{ mb: 4 }}>
            <Typography variant="h6" sx={{ mb: 2, fontWeight: 600 }}>
                Зарплата
            </Typography>
            <Box sx={{ px: 1 }}>
                <Slider
                    value={value}
                    onChange={(_e, newValue) => onChange(newValue as number[])}
                    valueLabelDisplay="auto"
                    min={0}
                    max={1000000}
                    step={10000}
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
                    }}
                />
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
                    <Typography variant="body2">От: {value[0].toLocaleString()}</Typography>
                    <Typography variant="body2">До: {value[1].toLocaleString()}</Typography>
                </Box>
            </Box>
        </Box>
    );
};
