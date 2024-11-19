import { useEffect, useState } from "react";
import { useLogs } from "../hooks/useLogs";
import { Pagination } from "../../../shared/components/Pagination";
import { LogRowItem } from "../components";

export const LogPage = () => {
  const {logs, loadLogs, isLoading} = useLogs();
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [fetching, setFetching] = useState(true);

  useEffect(() => {
    if (fetching) {
      loadLogs(searchTerm, currentPage);
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
    if (logs.data.hasPreviousPage) {
      setCurrentPage((prevPage) => prevPage - 1);
      setFetching(true);
    }
  };

  // Ir a página siguiente
  const handleNextPage = () => {
    if (logs.data.hasNextPage) {
      setCurrentPage((prevPage) => prevPage + 1);
      setFetching(true);
    }
  };

  return (
    <div className="flex flex-col items-center w-full h-full p-8 bg-gray-100">
    <div className="w-full max-w-5xl p-6 bg-white rounded-lg shadow-md">
      <div className="flex items-center justify-between pb-4 border-b">
        <h2 className="text-2xl font-bold text-gray-800">Historial de Registros</h2>
        <form onSubmit={handleSubmit}>
          <div className="flex">
            <input
              type="search"
              placeholder="Buscar registro..."
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
              <th className="px-4 py-2 text-left text-gray-600 border-b">Acción</th>
              <th className="px-4 py-2 text-left text-gray-600 border-b">Descripción</th>
              <th className="px-4 py-2 text-left text-gray-600 border-b">Usuario</th>
              <th className="px-4 py-2 text-left text-gray-600 border-b">Fecha</th>
            </tr>
          </thead>
          <tbody>
          {isLoading ? (
              <tr>
                <td colSpan="5" className="px-4 py-2 text-center text-gray-500">
                  Cargando...
                </td>
              </tr>
            ) : logs?.data?.items?.length ? (
              logs.data.items.map((log) => (
                <LogRowItem key={log.id} log={log} />
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
      totalPages={logs?.data?.totalPages}
      hasNextPage={logs?.data?.hasNextPage}
      hasPreviousPage={logs?.data?.hasPreviousPage}
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

