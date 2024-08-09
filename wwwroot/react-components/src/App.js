import { useEffect } from 'react';
import { Toaster, toast } from 'sonner';

function App() {
    useEffect(() => {
        window.toast = toast;
    }, [])

    return (
        <div className="App">
            <Toaster
                theme="light"
                richColors
                position="bottom-right"
            />
        </div>
    );
}

export default App;
