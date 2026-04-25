import Box from '@mui/material/Box';
import Checkbox from '@mui/material/Checkbox';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormControl from '@mui/material/FormControl';
import FormLabel from '@mui/material/FormLabel';
import FormGroup from '@mui/material/FormGroup';

interface ScheduleCheckboxGroupProps {
    values: string[];
    onChange: (value: string, checked: boolean) => void;
}

export const ScheduleCheckboxGroup = ({ values, onChange }: ScheduleCheckboxGroupProps) => {
    return (
        <Box>
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
                    Формат работы
                </FormLabel>
                <FormGroup>
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={values.includes('remote')}
                                onChange={(e) => onChange('remote', e.target.checked)}
                                sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }}
                            />
                        }
                        label="Удаленно"
                        sx={{ color: 'white', mb: 1 }}
                    />
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={values.includes('fullDay')}
                                onChange={(e) => onChange('fullDay', e.target.checked)}
                                sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }}
                            />
                        }
                        label="На месте работодателя"
                        sx={{ color: 'white', mb: 1 }}
                    />
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={values.includes('flexible')}
                                onChange={(e) => onChange('flexible', e.target.checked)}
                                sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }}
                            />
                        }
                        label="Гибрид"
                        sx={{ color: 'white' }}
                    />
                </FormGroup>
            </FormControl>
        </Box>
    );
};
