import { Component, type ErrorInfo, type ReactNode } from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';

interface Props {
    children: ReactNode;
}

interface State {
    hasError: boolean;
    error: Error | null;
}

export class ErrorBoundary extends Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = {
            hasError: false,
            error: null,
        };
    }

    static getDerivedStateFromError(error: Error): State {
        return {
            hasError: true,
            error,
        };
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo) {
        console.error('ErrorBoundary caught an error:', error, errorInfo);
    }

    handleReset = () => {
        this.setState({
            hasError: false,
            error: null,
        });
        window.location.href = '/';
    };

    render() {
        if (this.state.hasError) {
            return (
                <Box
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        justifyContent: 'center',
                        minHeight: '100vh',
                        bgcolor: '#0f0c5f',
                        color: 'white',
                        px: 3,
                        textAlign: 'center',
                    }}
                >
                    <Typography variant="h3" sx={{ mb: 2, fontWeight: 700 }}>
                        Что-то пошло не так
                    </Typography>
                    <Typography variant="body1" sx={{ mb: 4, maxWidth: 600 }}>
                        Произошла непредвиденная ошибка. Попробуйте обновить страницу или вернуться на главную.
                    </Typography>
                    {this.state.error && (
                        <Typography
                            variant="body2"
                            sx={{
                                mb: 4,
                                p: 2,
                                bgcolor: 'rgba(255, 255, 255, 0.1)',
                                borderRadius: 1,
                                fontFamily: 'monospace',
                                maxWidth: 800,
                                overflow: 'auto',
                            }}
                        >
                            {this.state.error.toString()}
                        </Typography>
                    )}
                    <Box sx={{ display: 'flex', gap: 2 }}>
                        <Button
                            variant="contained"
                            onClick={() => window.location.reload()}
                            sx={{
                                bgcolor: 'white',
                                color: '#0f0c5f',
                                '&:hover': { bgcolor: '#f0f0f0' },
                            }}
                        >
                            Обновить страницу
                        </Button>
                        <Button
                            variant="outlined"
                            onClick={this.handleReset}
                            sx={{
                                borderColor: 'white',
                                color: 'white',
                                '&:hover': { borderColor: '#f0f0f0', bgcolor: 'rgba(255, 255, 255, 0.1)' },
                            }}
                        >
                            На главную
                        </Button>
                    </Box>
                </Box>
            );
        }

        return this.props.children;
    }
}
