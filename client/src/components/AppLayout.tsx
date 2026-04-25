import { Outlet } from 'react-router-dom';
import { Header } from '../modules/Header';

export const AppLayout = () => {
    return (
        <>
            <Header />
            <Outlet />
        </>
    );
};
