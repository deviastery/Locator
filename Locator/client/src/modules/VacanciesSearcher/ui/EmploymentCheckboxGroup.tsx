import Box from '@mui/material/Box';
import Checkbox from '@mui/material/Checkbox';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormControl from '@mui/material/FormControl';
import FormLabel from '@mui/material/FormLabel';
import FormGroup from '@mui/material/FormGroup';

interface EmploymentCheckboxGroupProps {
    values: string[];
    onChange: (value: string, checked: boolean) => void;
}

export const EmploymentCheckboxGroup = ({ values, onChange }: EmploymentCheckboxGroupProps) => {
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
                    Тип занятости
                </FormLabel>
                <FormGroup>
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={values.includes('full')}
                                onChange={(e) => onChange('full', e.target.checked)}
                                sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }}
                            />
                        }
                        label="Полная занятость"
                        sx={{ color: 'white', mb: 1 }}
                    />
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={values.includes('part')}
                                onChange={(e) => onChange('part', e.target.checked)}
                                sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }}
                            />
                        }
                        label="Частичная занятость"
                        sx={{ color: 'white', mb: 1 }}
                    />
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={values.includes('project')}
                                onChange={(e) => onChange('project', e.target.checked)}
                                sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }}
                            />
                        }
                        label="Проектная работа"
                        sx={{ color: 'white', mb: 1 }}
                    />
                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={values.includes('probation')}
                                onChange={(e) => onChange('probation', e.target.checked)}
                                sx={{ color: 'white', '&.Mui-checked': { color: 'white' } }}
                            />
                        }
                        label="Стажировка"
                        sx={{ color: 'white' }}
                    />
                </FormGroup>
            </FormControl>
        </Box>
    );
};
