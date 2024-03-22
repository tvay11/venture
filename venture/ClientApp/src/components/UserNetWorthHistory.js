import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Line } from 'react-chartjs-2';
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend } from 'chart.js';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend);

const UserNetWorthGraph = () => {
    const [netWorthData, setNetWorthData] = useState([]);


    const fetchNetWorthData = async () => {
        try {
            const response = await axios.get('http://localhost:8080/api/stock/user/1/history');
            setNetWorthData(response.data.$values); 
        } catch (error) {
            console.error('Failed to fetch net worth data:', error);
        }
    };
    useEffect(() => {
        fetchNetWorthData();
    }, []);

    const data = {
        labels: netWorthData.map(item => new Date(item.date).toLocaleDateString()),
        datasets: [
            {
                label: 'Net Worth',
                data: netWorthData.map(item => item.netWorth),
                fill: false,
                backgroundColor: 'rgb(75, 192, 192)',
                borderColor: 'rgba(75, 192, 192, 0.2)',
            },
        ],
    };

    const options = {
        scales: {
            y: {
                beginAtZero: false,
            },
        },
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'User Net Worth History',
            },
        },
    };

    return <Line data={data} options={options} />;
};

export default UserNetWorthGraph;
