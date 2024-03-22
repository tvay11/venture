import React, { useState } from 'react';
import axios from 'axios';
import {Input, Table, Button, InputNumber} from 'antd';

const SearchStocks = () => {
    const [keyword, setKeyword] = useState('');
    const [searchResults, setSearchResults] = useState([]);
    const [loading, setLoading] = useState(false);
    const [inputValues, setInputValues] = useState({});
    const userId = 1; 

    const handleSearch = async () => {
        setLoading(true);
        try {
            const response = await axios.get(`http://localhost:8080/api/stock/search/${keyword}`);
            console.log(response.data);
            setSearchResults(response.data.$values);

        } catch (error) {
            console.error('Failed to search stocks:', error);
        } finally {
            setLoading(false);
        }
    };

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
            window.location.reload();
            
        } catch (error) {
            console.error('Error buying stock:', error);
            alert('Failed to buy stock.');
        }
    };


    const columns = [
        { title: 'Symbol', dataIndex: 'symbol', key: 'symbol' },
        { title: 'Name', dataIndex: 'name', key: 'name' },
        { title: 'Current Price', dataIndex: 'currentPrice', key: 'currentPrice' },
        {
            title: 'Action',
            key: 'action',
            render: (_, record) => (
                <div>
                    <Input
                        style={{ width: 50, margin: 8, height: 32 }}
                        value={inputValues[record.stockId] || ''}
                        onChange={(e) => handleInputChange(record.stockId, e.target.value)}
                    />
                    <Button
                        type="primary"
                        onClick={() => buyStock(record.stockId)}
                        style={{ backgroundColor: 'green', borderColor: 'green' }}
                    >
                        Buy
                    </Button>
                </div>
            ),
        },
    ];

    return (
        <div>
            <Input.Search
                value={keyword}
                onChange={(e) => setKeyword(e.target.value)}
                onSearch={handleSearch}
                placeholder="Search for stocks"
                enterButton
                loading={loading}
            />
            <Table dataSource={searchResults} columns={columns} rowKey="symbol" />
        </div>
    );
};

export default SearchStocks;
