import MuiPagination from "@mui/material/Pagination";
import React from "react";

interface CustomPaginationProps {
    totalPages: number;
    page: number;
    onChange: (event: React.ChangeEvent<unknown>, value: number) => void;
}

export const Pagination = ({ totalPages, page, onChange }: CustomPaginationProps) => {
    return (
        <MuiPagination
            count={totalPages}
            page={page}
            onChange={onChange}
            color="primary"
            size="large"
            showFirstButton
            showLastButton
            sx={{
                '& .MuiPaginationItem-root': {
                    color: 'white',
                },
                '& .MuiPaginationItem-root.Mui-selected': {
                    backgroundColor: '#1976d2',
                    color: 'white',
                },
            }}
        />
    );
};
