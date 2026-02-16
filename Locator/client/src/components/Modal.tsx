import Box from '@mui/material/Box';
import MuiModal from '@mui/material/Modal';
import type { ReactNode } from 'react';

interface ModalProps {
    open: boolean;
    onClose: () => void;
    children: ReactNode;
    maxWidth?: string | number;
}

export const Modal = ({ open, onClose, children, maxWidth = 800 }: ModalProps) => {
    return (
        <MuiModal
            open={open}
            onClose={onClose}
            sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
            }}
        >
            <Box
                sx={{
                    bgcolor: 'white',
                    borderRadius: 3,
                    boxShadow: 24,
                    maxWidth,
                    width: '90%',
                    maxHeight: '90vh',
                    p: 4,
                    position: 'relative',
                    outline: 'none',
                    overflow: 'hidden',
                }}
            >
                {children}
            </Box>
        </MuiModal>
    );
};
