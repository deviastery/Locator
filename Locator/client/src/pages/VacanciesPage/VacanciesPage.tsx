import { VacanciesList } from "../../modules/VacanciesList";
import { VacanciesSearcher } from "../../modules/VacanciesSearcher";
import Box from '@mui/material/Box';
import { useState } from "react";
import type { GetVacanciesDto } from "../../contracts/dtos/GetVacanciesDto.ts";


export const VacanciesPage = () => {
    const [searchQuery, setSearchQuery] = useState('');
    const [filters, setFilters] = useState<GetVacanciesDto>({});

    return (
        <Box
            sx={{
                maxWidth: 1200,
                mx: 'auto',
                px: 3,
                py: 3,
            }}
        >
            <VacanciesSearcher
                searchQuery={searchQuery}
                setSearchQuery={setSearchQuery}
                onFiltersChange={setFilters}
            />
            <VacanciesList
                searchQuery={searchQuery}
                filters={filters}
            />
        </Box>
    );
}
