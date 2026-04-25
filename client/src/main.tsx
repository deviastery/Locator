import { StrictMode } from 'react'
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { createRoot } from 'react-dom/client'
import './index.css'
import { createBrowserRouter, RouterProvider, Navigate } from "react-router-dom"
import { AuthPage } from './pages/AuthPage';
import { VacanciesPage } from "./pages/VacanciesPage";
import { NegotiationsPage } from "./pages/NegotiationsPage";
import { AppLayout } from './components/AppLayout';
import { ErrorBoundary } from './components/ErrorBoundary';
import { TestError } from './components/TestError.tsx';
import { ErrorFallback } from './components/ErrorFallback';

const router = createBrowserRouter([
    {
        path: '/auth',
        element: <AuthPage />,
        errorElement: <ErrorFallback />,
    },
    {
        path: '/test-error',
        element: <TestError />,
        errorElement: <ErrorFallback />,
    },
    {
        path: '/',
        element: <AppLayout />,
        errorElement: <ErrorFallback />,
        children: [
            {
                index: true,
                element: <Navigate to="/auth" replace />,
            },
            {
                path: 'vacancies',
                element: <VacanciesPage />,
                errorElement: <ErrorFallback />,
            },
            {
                path: 'negotiations',
                element: <NegotiationsPage />,
                errorElement: <ErrorFallback />,
            },
        ],
    },
]);

const theme = createTheme({
    palette: {
        background: {
            default: '#030024',
        },
    },
});

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ErrorBoundary>
            <ThemeProvider theme={theme}>
                <CssBaseline />
                <RouterProvider router={router} />
            </ThemeProvider>
        </ErrorBoundary>
    </StrictMode>,
);
