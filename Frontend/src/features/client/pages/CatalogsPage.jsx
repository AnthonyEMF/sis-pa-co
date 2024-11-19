import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useAccounts } from "../hooks/useAccounts";
import { Pagination } from "../../../shared/components/Pagination";
import { AccountRowItem } from "../components";

export const CatalogsPage = () => {
  const {accounts, loadPaginationAccounts, isLoading} = useAccounts();
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [fetching, setFetching] = useState(true);

  useEffect(() => {
    if (fetching) {
      loadPaginationAccounts(searchTerm, currentPage);
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
    if (accounts.data.hasPreviousPage) {
      setCurrentPage((prevPage) => prevPage - 1);
      setFetching(true);
    }
  };

  // Ir a página siguiente
  const handleNextPage = () => {
    if (accounts.data.hasNextPage) {
      setCurrentPage((prevPage) => prevPage + 1);
      setFetching(true);
    }
  };

  return (
    <div className="relative flex flex-col items-center w-full h-full p-8 bg-gray-100">
      <div className="w-full max-w-5xl p-6 bg-white rounded-lg shadow-md">
        <div className="flex items-center justify-between pb-4 border-b">
          <h2 className="text-2xl font-bold text-gray-800">Catálogo de Cuentas</h2>
          <form onSubmit={handleSubmit}>
            <div className="flex">
              <input
                type="search"
                placeholder="Buscar cuenta..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="px-4 py-2 border rounded-r-none rounded-lg focus:outline-none focus:border-gray-500"
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
                <th className="px-4 py-2 text-left text-gray-600 border-b">CÓDIGO DE CUENTA</th>
                <th className="px-4 py-2 text-left text-gray-600 border-b">NOMBRE</th>
                <th className="px-4 py-2 text-left text-gray-600 border-b">MOVIMIENTO</th>
                <th className="px-4 py-2 text-left text-gray-600 border-b">ESTADO</th>
                <th className="px-4 py-2 text-left text-gray-600 border-b">ACCIÓN</th>
              </tr>
            </thead>
            <tbody>
            {isLoading ? (
              <tr>
                <td colSpan="5" className="px-4 py-2 text-center text-gray-500">
                  Cargando...
                </td>
              </tr>
            ) : accounts?.data?.items?.length ? (
              accounts.data.items.map((account) => (
                <AccountRowItem key={account.id} account={account} />
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
      <Link
        className="fixed bottom-8 right-8 bg-green-600 text-white rounded-full p-4 shadow-lg hover:bg-green-700"
        to="/post/account"
      >
        + Crear Nueva Cuenta
      </Link>

      {/* Paginación */}
      <div className="mt-6 mb-6">
        <Pagination
          totalPages={accounts?.data?.totalPages}
          hasNextPage={accounts?.data?.hasNextPage}
          hasPreviousPage={accounts?.data?.hasPreviousPage}
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
