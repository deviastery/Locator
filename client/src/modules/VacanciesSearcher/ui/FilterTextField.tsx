import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import TextField from '@mui/material/TextField';

interface FilterTextFieldProps {
    label: string;
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
}

export const FilterTextField = ({ label, value, onChange, placeholder }: FilterTextFieldProps) => {
    return (
        <Box sx={{ mb: 4 }}>
            <Typography variant="h6" sx={{ mb: 2, fontWeight: 600 }}>
                {label}
            </Typography>
            <TextField
                fullWidth
                placeholder={placeholder}
                variant="outlined"
                value={value}
                onChange={(e) => onChange(e.target.value)}
                sx={{
                    bgcolor: 'white',
                    borderRadius: 1,
                    '& .MuiOutlinedInput-root': {
                        '& fieldset': {
                            borderColor: 'transparent',
                        },
                    },
                }}
            />
        </Box>
    );
};
