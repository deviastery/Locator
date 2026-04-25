import Box from '@mui/material/Box';
import { useState } from 'react';
import { VacanciesFilter } from './components/VacanciesFilter';
import type { GetVacanciesDto } from "../../contracts/dtos/GetVacanciesDto.ts";
import { FilterButton } from './ui/FilterButton';
import { SearchTextField } from './ui/SearchTextField';

interface SearchBarProps {
    searchQuery: string;
    setSearchQuery: (query: string) => void;
    onFiltersChange: (filters: GetVacanciesDto) => void;
}

export const VacanciesSearcher = ({ searchQuery, setSearchQuery, onFiltersChange }: SearchBarProps) => {
    const [drawerOpen, setDrawerOpen] = useState(false);

    return (
        <>
            <Box
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: 0,
                    maxWidth: 1200,
                    mx: 'auto',
                    mb: 4,
                    px: 3,
                }}
            >
                <FilterButton onClick={() => setDrawerOpen(true)} />
                <SearchTextField
                    value={searchQuery}
                    onChange={setSearchQuery}
                    placeholder="Поиск по вакансиям"
                />
            </Box>

            <VacanciesFilter
                open={drawerOpen}
                onClose={() => setDrawerOpen(false)}
                onApply={onFiltersChange}
            />
        </>
    );
};
