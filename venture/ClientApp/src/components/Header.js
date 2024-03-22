import React, { useState, useEffect } from 'react';
import { Button } from 'antd';
import axios from 'axios';

const Header = ({ onDateChange }) => {
    const [currentDate, setCurrentDate] = useState('');
    const [cash, setCash] = useState(0);
    const [netWorth, setNetWorth] = useState(0);

    const fetchUserProfile = async () => {
        try {
            const response = await axios.get('http://localhost:8080/api/stock/user/1/profile');
            const data = response.data;
            setCurrentDate(new Date(data.currentDate).toLocaleDateString());
            setCash(data.cash);
            setNetWorth(data.netWorth);
        } catch (error) {
            console.error('Error fetching user profile:', error);
        }
    };

    useEffect(() => {
        fetchUserProfile();
    }, []);

    const handleNextDay = async () => {
        try {
            await axios.post('http://localhost:8080/api/stock/user/1/nextday');
            await fetchUserProfile();
            if (onDateChange) {
                onDateChange();  
            }
        } catch (error) {
            console.error('Error moving to the next day:', error);
        }
    };

    return (
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '20px', backgroundColor: '#f0f2f5' }}>
            <div>
                <h1 style={{ margin: 0 }}>Capital Venture</h1>
                <span>{`Cash: $${cash.toLocaleString()} | Net Worth: $${netWorth.toLocaleString()}`}</span>
            </div>
            <div>
                <span>{currentDate}</span>
                <Button onClick={handleNextDay} style={{ marginLeft: '20px' }}>Next</Button>
            </div>
        </div>
    );
};

export default Header;
