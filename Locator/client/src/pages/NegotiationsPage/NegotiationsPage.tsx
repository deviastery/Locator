import Box from '@mui/material/Box';
import { NegotiationsList } from '../../modules/NegotiationsList';


export const NegotiationsPage = () => {
    return (
        <Box             
            sx={{
            maxWidth: 1200,
            mx: 'auto',
            px: 3,
            py: 3,
        }}>
            <NegotiationsList />
        </Box>
    );
}
