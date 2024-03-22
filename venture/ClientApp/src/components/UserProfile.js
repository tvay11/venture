import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Card, Spin, Alert, Table, Input, Button } from 'antd';

const UserProfile = () => {
    const [profile, setProfile] = useState(null);
    const [stocks, setStocks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [inputValues, setInputValues] = useState({});
    const userId = 1;
    
    const fetchData = async () => {
        try {
            const profileResponse = await axios.get(`http://localhost:8080/api/stock/user/1/profile`);
            setProfile(profileResponse.data);

            const stocksResponse = await axios.get(`http://localhost:8080/api/stock/user/1/stockinfo`);
            setStocks(stocksResponse.data.$values);
        } catch (error) {
            console.error('Error fetching data:', error);
            setError(error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleInputChange = (stockId, value) => {
        setInputValues({ ...inputValues, [stockId]: value });
    };

    const buyStock = async (stockId) => {
        const quantity = inputValues[stockId];
        if (!quantity) {
            alert('Please enter a quantity to buy.');
            return;
        }

        try {

            await axios.post(`http://localhost:8080/api/stock/user/${userId}/buy/${stockId}/${quantity}`);
            await fetchData();
        } catch (error) {
            console.error('Error buying stock:', error);
            alert('Failed to buy stock.');
        }
    };

    const sellStock = async (stockId) => {
        const quantity = inputValues[stockId];
        if (!quantity) {
            alert('Please enter a quantity to sell.');
            return;
        }

        try {
            await axios.post(`http://localhost:8080/api/stock/user/${userId}/sell/${stockId}/${quantity}`);
            await fetchData();
        } catch (error) {
            console.error('Error selling stock:', error);
            alert('Failed to sell stock.');
        }
    };

    const stockColumns = [
        // { title: 'Stock ID', dataIndex: 'stockId', key: 'stockId' },
        { title: 'Symbol', dataIndex: 'symbol', key: 'symbol' },
        { title: 'Stock Name', dataIndex: 'name', key: 'name' },
        { title: 'Quantity', dataIndex: 'quantity', key: 'quantity' },
        { title: 'Price', dataIndex: 'currentPrice', key: 'currentPrice' },
        // { title: 'Date', dataIndex: 'currentDate', key: 'currentDate', render: text => new Date(text).toLocaleDateString() },
        {
            title: 'Action',
            key: 'action',
            render: (text, record) => (
                <div>
                    <Input
                        style={{ width: 50, margin: 8, height: 32 }}
                        value={inputValues[record.stockId] || ''}
                        onChange={(e) => handleInputChange(record.stockId, e.target.value)}
                    />
                    <Button type="primary" onClick={() => buyStock(record.stockId, inputValues[record.stockId])} style={{ margin: 8, backgroundColor: 'green', borderColor: 'green' }}>
                        Buy
                    </Button>
                    <Button type="primary"
                        className="sellButton"
                        onClick={() => sellStock(record.stockId, inputValues[record.stockId])}
                        style={{ margin: 8, backgroundColor: 'red', borderColor: 'red' }}
                    >
                        Sell
                    </Button>
                </div>
            ),
        },
    ];

    if (loading) {
        return <Spin tip="Loading..." />;
    }

    if (error) {
        return <Alert message="Error" description="Failed to load data." type="error" showIcon />;
    }

    return (
        <div style={{ padding: '20px' }}>
            {profile && (
                <>
                    <Card title="Stock Holdings" bordered={false}>
                        <Table columns={stockColumns} dataSource={stocks} pagination={false} />
                    </Card> 
                </>
            )}
        </div>
    );
};

export default UserProfile;
