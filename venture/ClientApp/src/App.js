import React, {useState} from 'react';
import Header from './components/Header'; 
import UserProfile from './components/UserProfile';
import SearchStocks from './components/SearchStocks';
import './App.css';
import UserNetWorthHistory from "./components/UserNetWorthHistory";

const App = () => {

    const [key, setKey] = useState(0);  

    const refreshData = () => {
        setKey(prevKey => prevKey + 1);  
    };
    
    return (
        <div className="app-container">
            <Header  /> 
            <main className="content">
                <UserProfile  />
                <SearchStocks />
            </main>
        </div>
    );
};

export default App;
