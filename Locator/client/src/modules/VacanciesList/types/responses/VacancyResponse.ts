import type { Review } from '../Review.ts';
import type { FullVacancyDto } from "../../../../contracts/dtos/FullVacancyDto.ts";

export interface VacancyResponse {
    vacancy: FullVacancyDto;
    reviews: Review[];
}