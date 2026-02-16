import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import type { ReactNode } from "react";

interface CenteredMessageProps {
    children: ReactNode;
}

export const CenteredBox = ({ children }: CenteredMessageProps) => {
    return (
        <Box
            sx={{
                maxWidth: 1200,
                mx: 'auto',
                px: 3,
                py: 10,
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center'
            }}
        >
            <Typography variant="h5" sx={{ color: 'white', textAlign: 'center' }}>
                {children}
            </Typography>
        </Box>
    );
};
