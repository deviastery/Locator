import Box from '@mui/material/Box';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormControl from '@mui/material/FormControl';
import FormLabel from '@mui/material/FormLabel';

interface ExperienceRadioGroupProps {
    value: string;
    onChange: (value: string) => void;
}

export const ExperienceRadioGroup = ({ value, onChange }: ExperienceRadioGroupProps) => {
    return (
        <Box sx={{ mb: 4 }}>
            <FormControl component="fieldset" fullWidth>
                <FormLabel
                    component="legend"
                    sx={{
                        color: 'white',
                        fontWeight: 600,
                        mb: 2,
                        fontSize: '1.25rem',
                        '&.Mui-focused': {
                            color: 'white',
                        },
                    }}
                >
                    Опыт работы
                </FormLabel>
                <RadioGroup value={value} onChange={(e) => onChange(e.target.value)}>
                    <FormControlLabel
                        value="noExperience"
                        control={<Radio sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }} />}
                        label="Без опыта"
                        sx={{ color: 'white', mb: 1 }}
                    />
                    <FormControlLabel
                        value="between1And3"
                        control={<Radio sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }} />}
                        label="От 1 года до 3 лет"
                        sx={{ color: 'white', mb: 1 }}
                    />
                    <FormControlLabel
                        value="between3And6"
                        control={<Radio sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }} />}
                        label="От 3 до 6 лет"
                        sx={{ color: 'white', mb: 1 }}
                    />
                    <FormControlLabel
                        value="moreThan6"
                        control={<Radio sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }} />}
                        label="От 6 лет"
                        sx={{ color: 'white' }}
                    />
                </RadioGroup>
            </FormControl>
        </Box>
    );
};
