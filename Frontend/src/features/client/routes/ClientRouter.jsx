import { Navigate, Route, Routes } from 'react-router-dom';
import {HomePage, TransactionsPage, BalancePage, CatalogsPage, LogPage } from '../pages';
import { Nav } from '../components/Nav';
import { AccountCreate, TransactionCreate, TransactionDetails, AccountDetails } from '../components';

export const ClientRouter = () => {
  return (
    <div className="overflow-x-hidden bg-gray-100 w-screen h-screen bg-hero-pattern bg-no-repeat bg-cover">
      <Nav />
        <div className="px-6">
        <div className="container flex justify-between mx-auto">
            <Routes>
                <Route path='/home' element={<HomePage/>}/>
                <Route path='/transactions' element={<TransactionsPage/>}/>
                <Route path='/balances' element={<BalancePage/>}/>
                <Route path='/catalogs' element={<CatalogsPage/>}/>
                <Route path='/logs' element={<LogPage />}/>
                <Route path='/post/transactions' element={<TransactionCreate />}/>
                <Route path='/post/account' element={<AccountCreate />}/>
                <Route path='/transactions-details/:id' element={<TransactionDetails />}/>
                <Route path='/accounts-details/:id' element={<AccountDetails />}/>
                <Route path='/*' element={<Navigate to={"/home"} />} />
            </Routes>
        </div>
      </div> 
    </div>
  )
}