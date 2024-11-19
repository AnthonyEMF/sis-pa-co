import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useTransactions } from '../hooks/useTransactions';
import { Pagination } from '../../../shared/components/Pagination';
import { TransactionRowItem } from '../components';

export const TransactionsPage = () => {
  const {transactions, loadTransactions, isLoading} = useTransactions();
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [fetching, setFetching] = useState(true);

  useEffect(() => {
    if (fetching) {
      loadTransactions(searchTerm, currentPage);
      setFetching(false);
    }
  }, [fetching, searchTerm, currentPage]);

  const handleSubmit = (e) => {
    e.preventDefault();
    setFetching(true);
  };

  // Cambiar a una página especifica
  const handleCurrentPage = (index = 1) => {
    setCurrentPage(index);
    setFetching(true);
  };

  // Ir a página anterior
  const handlePreviousPage = () => {
    if (transactions.data.hasPreviousPage) {
      setCurrentPage((prevPage) => prevPage - 1);
      setFetching(true);
    }
  };

  // Ir a página siguiente
  const handleNextPage = () => {
    if (transactions.data.hasNextPage) {
      setCurrentPage((prevPage) => prevPage + 1);
      setFetching(true);
    }
  };

  return (
    <div className="flex flex-col items-center w-full h-full p-8 bg-gray-100">
      <div className="w-full max-w-5xl p-6 bg-white rounded-lg shadow-md">
        <div className="flex items-center justify-between pb-4 border-b">
          <h1 className="text-2xl font-bold text-gray-800">Partidas Contables</h1>
          <form onSubmit={handleSubmit}>
            <div className="flex">

              <input
                type="text"
                placeholder="Buscar partida..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="px-4 py-2 border rounded-lg rounded-r-none focus:outline-none focus:border-gray-500"
                />
                <button
                  type="submit"
                  className="bg-gray-600 text-white px-4 py-2 rounded-r-md hover:bg-gray-500"
                > Buscar
                </button>
            </div>
            </form>
        </div>
        
        <table className="min-w-full divide-y divide-gray-200 mt-3">
          <thead>
            <tr>
              <th className="px-4 py-2 text-left text-gray-600 border-b">
                NÚMERO
              </th>
              <th className="px-4 py-2 text-left text-gray-600 border-b">
                FECHA
              </th>
              <th className="px-4 py-2 text-left text-gray-600 border-b">
                DESCRIPCIÓN
              </th>
              <th className="px-4 py-2 text-left text-gray-600 border-b">
                USUARIO
              </th>
              <th className="px-4 py-2 text-left text-gray-600 border-b">
                ESTADO
              </th>
              <th className="px-4 py-2 text-left text-gray-600 border-b">
                DETALLES
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
          {isLoading ? (
              <tr>
                <td colSpan="5" className="px-4 py-2 text-center text-gray-500">
                  Cargando...
                </td>
              </tr>
            ) : transactions?.data?.items?.length ? (
              transactions.data.items.map((transaction) => (
                <TransactionRowItem key={transaction.id} transaction={transaction} />
              ))
            ) : (
              <tr>
                <td colSpan="5" className="px-4 py-2 text-center text-gray-500">
                  No se encontraron resultados.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Link
        className="fixed bottom-8 right-8 bg-green-600 text-white rounded-full p-4 shadow-lg hover:bg-green-700"
        to="/post/transactions"
      >
        + Crear Nueva Partida
      </Link>

      {/* Paginación */}
      <div className="mt-6 mb-6">
        <Pagination
          totalPages={transactions?.data?.totalPages}
          hasNextPage={transactions?.data?.hasNextPage}
          hasPreviousPage={transactions?.data?.hasPreviousPage}
          currentPage={currentPage}
          handleNextPage={handleNextPage}
          handlePreviousPage={handlePreviousPage}
          setCurrentPage={setCurrentPage}
          handleCurrentPage={handleCurrentPage}
        />
      </div>
    </div>
  );
};
