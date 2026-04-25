import Box from '@mui/material/Box';
import Drawer from '@mui/material/Drawer';
import Typography from '@mui/material/Typography';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import { useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import type { GetVacanciesDto } from '../../../contracts/dtos/GetVacanciesDto';
import { FilterTextField } from '../ui/FilterTextField';
import { SalarySlider } from '../ui/SalarySlider';
import { RatingSlider } from '../ui/RatingSlider';
import { OnlyWithReviewsCheckbox } from '../ui/OnlyWithReviewsCheckbox';
import { ExperienceRadioGroup } from '../ui/ExperienceRadioGroup';
import { EmploymentCheckboxGroup } from '../ui/EmploymentCheckboxGroup';
import { ScheduleCheckboxGroup } from '../ui/ScheduleCheckboxGroup';
import { ApplyFilterButton } from '../ui/ApplyFilterButton';
import { ResetFilterButton } from "../ui/ResetFilterButton";

interface VacanciesFilterProps {
    open: boolean;
    onClose: () => void;
    onApply: (filters: GetVacanciesDto) => void;
}

export const VacanciesFilter = ({ open, onClose, onApply }: VacanciesFilterProps) => {
    const [, setSearchParams] = useSearchParams();
    const [searchField, setSearchField] = useState<string>('');
    const [experience, setExperience] = useState<string>('');
    const [employment, setEmployment] = useState<string[]>([]);
    const [schedule, setSchedule] = useState<string[]>([]);
    const [area, setArea] = useState<string>('');
    const [salaryRange, setSalaryRange] = useState<number[]>([0, 1000000]);
    const [ratingRange, setRatingRange] = useState<number[]>([0, 5]);
    const [onlyWithReviews, setOnlyWithReviews] = useState<boolean>(false);

    const resetPageToFirst = () => {
        localStorage.setItem('vacanciesPage', '1');
        setSearchParams({ page: '1' }, { replace: true });
    };

    const handleApplyFilters = () => {
        const filters: GetVacanciesDto = {};

        if (searchField) filters.searchField = searchField;
        if (experience) filters.experience = experience;
        if (employment.length > 0) filters.employment = employment.join(',');
        if (schedule.length > 0) filters.schedule = schedule.join(',');
        if (area) filters.area = area;

        const salaryFrom = salaryRange[0];
        const salaryTo = salaryRange[1];

        if (salaryFrom > 0 || salaryTo < 1000000) {
            filters.salary = {
                from: salaryFrom > 0 ? salaryFrom : undefined,
                to: salaryTo < 1000000 ? salaryTo : undefined,
                currency: 'RUR',
            };
        }

        if (ratingRange[0] > 0) filters.minRating = ratingRange[0];
        if (ratingRange[1] < 5) filters.maxRating = ratingRange[1];
        if (onlyWithReviews) filters.onlyWithReviews = true;

        resetPageToFirst();
        onApply(filters);
        onClose();
    };

    const handleResetFilters = () => {
        setSearchField('');
        setExperience('');
        setEmployment([]);
        setSchedule([]);
        setArea('');
        setSalaryRange([0, 1000000]);
        setRatingRange([0, 5]);
        setOnlyWithReviews(false);

        resetPageToFirst();
        onApply({});
        onClose();
    };

    const handleEmploymentChange = (value: string, checked: boolean) => {
        setEmployment(prev =>
            checked ? [...prev, value] : prev.filter(v => v !== value)
        );
    };

    const handleScheduleChange = (value: string, checked: boolean) => {
        setSchedule(prev =>
            checked ? [...prev, value] : prev.filter(v => v !== value)
        );
    };

    return (
        <Drawer
            anchor="left"
            open={open}
            onClose={onClose}
            transitionDuration={500}
        >
            <Box
                sx={{
                    width: 400,
                    height: '100%',
                    overflowY: 'auto',
                    p: 3,
                    display: 'flex',
                    flexDirection: 'column',
                    bgcolor: '#0f0c5f',
                    color: 'white',
                }}
            >
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                    <Typography variant="h5" sx={{ fontWeight: 700 }}>
                        Фильтры
                    </Typography>
                    <IconButton onClick={onClose} sx={{ color: 'white' }}>
                        <CloseIcon />
                    </IconButton>
                </Box>

                <FilterTextField
                    label="Название вакансии"
                    value={searchField}
                    onChange={setSearchField}
                    placeholder="Например: Frontend Developer"
                />

                <FilterTextField
                    label="Город"
                    value={area}
                    onChange={setArea}
                    placeholder="Москва"
                />

                <SalarySlider value={salaryRange} onChange={setSalaryRange} />

                <RatingSlider value={ratingRange} onChange={setRatingRange} />

                <OnlyWithReviewsCheckbox checked={onlyWithReviews} onChange={setOnlyWithReviews} />

                <ExperienceRadioGroup value={experience} onChange={setExperience} />

                <EmploymentCheckboxGroup values={employment} onChange={handleEmploymentChange} />

                <ScheduleCheckboxGroup values={schedule} onChange={handleScheduleChange} />

                <ApplyFilterButton onClick={handleApplyFilters} />
                <ResetFilterButton onClick={handleResetFilters} />
            </Box>
        </Drawer>
    );
};
