import Box from "@mui/material/Box";
import type { ReactNode } from "react";
import { Pagination } from "../ui/Pagination";

interface ListWithPaginationProps {
    children: ReactNode;
    totalPages: number;
    page: number;
    onChange: (event: React.ChangeEvent<unknown>, value: number) => void;
}

export const ListWithPagination = ({ children, totalPages, page, onChange }: ListWithPaginationProps) => {
    return (
        <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3 }}>
            {children}

            {totalPages > 1 && (
                <Box
                    sx={{
                        display: 'flex',
                        justifyContent: 'center',
                        mt: 4,
                        mb: 2,
                    }}
                >
                    <Pagination
                        totalPages={totalPages}
                        page={page}
                        onChange={onChange}
                    />
                </Box>
            )}
        </Box>
    );
};
