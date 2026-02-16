import TextField from '@mui/material/TextField';

interface SearchTextFieldProps {
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
}

export const SearchTextField = ({ value, onChange, placeholder = "Поиск по вакансиям" }: SearchTextFieldProps) => {
    return (
        <TextField
            fullWidth
            placeholder={placeholder}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            sx={{
                '& .MuiOutlinedInput-root': {
                    borderRadius: '0 12px 12px 0',
                    height: 56,
                    bgcolor: 'white',
                    '& fieldset': {
                        borderColor: '#e0e0e0',
                    },
                    '&:hover fieldset': {
                        borderColor: '#0f0c5f',
                    },
                    '&.Mui-focused fieldset': {
                        borderColor: '#0f0c5f',
                    },
                },
            }}
        />
    );
};
