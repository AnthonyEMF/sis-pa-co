import { useEffect, useState } from "react";
import { useBalances } from "../hooks/useBalances";
import { Pagination } from "../../../shared/components/Pagination";
import { BalanceRowItem } from "../components";

export const BalancePage = () => {
  const {balances, loadBalances, isLoading} = useBalances();
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [fetching, setFetching] = useState(true);

  useEffect(() => {
    if (fetching) {
      loadBalances(searchTerm, currentPage);
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
    if (balances.data.hasPreviousPage) {
      setCurrentPage((prevPage) => prevPage - 1);
      setFetching(true);
    }
  };

  // Ir a página siguiente
  const handleNextPage = () => {
    if (balances.data.hasNextPage) {
      setCurrentPage((prevPage) => prevPage + 1);
      setFetching(true);
    }
  };

  return (
    <div className="flex flex-col items-center w-full h-full p-8 bg-gray-100">
      <div className="w-full max-w-5xl p-6 bg-white rounded-lg shadow-md">
        <div className="flex items-center justify-between pb-4 border-b">
          <h2 className="text-2xl font-bold text-gray-800">Saldos de Cuentas</h2>
          <form onSubmit={handleSubmit}>
            <div className="flex">
              <input
                type="search"
                placeholder="Buscar cuenta..."
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
        <div className="mt-6 overflow-x-auto">
          <table className="min-w-full bg-white border border-gray-200 rounded-md">
            <thead>
              <tr>
                <th className="px-4 py-2 text-left text-gray-600 border-b">CÓDIGO DE SALDO</th>
                <th className="px-4 py-2 text-left text-gray-600 border-b">MES</th>
                <th className="px-4 py-2 text-left text-gray-600 border-b">AÑO</th>
                <th className="px-4 py-2 text-left text-gray-600 border-b">CUENTA</th>
                <th className="px-4 py-2 text-left text-gray-600 border-b">SALDO</th>
              </tr>
            </thead>
            <tbody>
            {isLoading ? (
              <tr>
                <td colSpan="5" className="px-4 py-2 text-center text-gray-500">
                  Cargando...
                </td>
              </tr>
            ) : balances?.data?.items?.length ? (
              balances.data.items.map((balance) => (
                <BalanceRowItem key={balance.id} balance={balance} />
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
      </div>
      {/* Paginación */}
      <div className="mt-6 mb-6">
        <Pagination
          totalPages={balances?.data?.totalPages}
          hasNextPage={balances?.data?.hasNextPage}
          hasPreviousPage={balances?.data?.hasPreviousPage}
          currentPage={currentPage}
          handleNextPage={handleNextPage}
          handlePreviousPage={handlePreviousPage}
          setCurrentPage={setCurrentPage}
          handleCurrentPage={handleCurrentPage}
        />
      </div>
    </div>
    )
  }
