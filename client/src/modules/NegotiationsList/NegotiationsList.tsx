import { NegotiationCard } from './components/NegotiationCard';
import { ListWithPagination } from "../../components/ListWithPagination";
import { useEffect, useState } from 'react';
import { fetchNegotiations } from './api/fetchNegotiations';
import type { Negotiation } from './types/Negotiation';
import { useSearchParams } from 'react-router-dom';
import { CenteredBox } from '../../components/CenteredBox';

export const NegotiationsList = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const [negotiations, setNegotiations] = useState<Negotiation[] | null>(null);
    const [loading, setLoading] = useState(true);
    const [page, setPage] = useState(() => {
        const pageFromUrl = searchParams.get('page');
        if (pageFromUrl) {
            return parseInt(pageFromUrl, 10);
        }
        const savedPage = localStorage.getItem('negotiationsPage');
        return savedPage ? parseInt(savedPage, 10) : 1;
    });
    const [totalPages, setTotalPages] = useState(0);
    const perPage = 5;

    useEffect(() => {
        setSearchParams({ page: page.toString() }, { replace: true });
        localStorage.setItem('negotiationsPage', page.toString());
    }, [page, setSearchParams]);

    useEffect(() => {
        const loadNegotiations = async () => {
            setLoading(true);

            const response = await fetchNegotiations({
                perPage: perPage,
                page: page,
            });

            if (response && 'unauthorized' in response && response.unauthorized) {
                setNegotiations([]);
            } else if (response && 'negotiations' in response) {
                setNegotiations(response.negotiations);
                setTotalPages(response.pages);
            } else {
                setNegotiations([]);
            }
            setLoading(false);
        };

        loadNegotiations();
    }, [page]);

    const handlePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
        setPage(value);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    if (loading || negotiations === null) {
        return <CenteredBox>Загрузка...</CenteredBox>;
    }

    if (negotiations.length === 0) {
        return <CenteredBox>У вас пока нет откликов</CenteredBox>;
    }

    return (
        <ListWithPagination
            totalPages={totalPages}
            page={page}
            onChange={handlePageChange}
        >
            {negotiations.map((n) => (
                <NegotiationCard key={n.id} negotiation={n} />
            ))}
        </ListWithPagination>
    );
};
