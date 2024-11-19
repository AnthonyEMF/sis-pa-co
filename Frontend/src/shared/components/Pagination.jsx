import { generateId } from "../utils";
import { FaArrowLeft, FaArrowRight } from "react-icons/fa";

export const Pagination = ({
    totalPages,
    handleCurrentPage,
    currentPage,
    handlePreviousPage = () => {},
    hasPreviousPage,
    handleNextPage = () => {},
    hasNextPage
}) => {
    const MAX_PAGES_VISIBLE = 5; // Número máximo de páginas visibles antes de usar "..."
    
    // Lógica para las páginas a mostrar
    const pages = [];
    if (totalPages <= MAX_PAGES_VISIBLE) {
        // Si hay menos o igual a MAX_PAGES_VISIBLE, mostrar todas las páginas
        for (let i = 1; i <= totalPages; i++) {
            pages.push(i);
        }
    } else {
        // Si hay más páginas, mostrar solo un número limitado
        if (currentPage <= 3) {
            // Si estamos cerca del inicio, mostrar las primeras páginas
            pages.push(1, 2, 3, '...', totalPages);
        } else if (currentPage >= totalPages - 2) {
            // Si estamos cerca del final, mostrar las últimas páginas
            pages.push(1, '...', totalPages - 2, totalPages - 1, totalPages);
        } else {
            // Mostrar las páginas alrededor de la página actual
            pages.push(1, '...', currentPage - 1, currentPage, currentPage + 1, '...', totalPages);
        }
    }

    return (
        <div className="flex justify-end space-x-2">
            {/* Anterior */}
            <button
                onClick={handlePreviousPage}
                disabled={!hasPreviousPage}
                className={`bg-gray-600 text-white px-4 py-2 rounded
                  ${!hasPreviousPage ? "cursor-not-allowed" : "hover:bg-gray-500"}`}
            >
                <FaArrowLeft />
            </button>

            {/* Numeración */}
            {pages.map((page) => (
                <button
                    key={generateId()}
                    onClick={() => (typeof page === 'number' ? handleCurrentPage(page) : null)}
                    className={`text-gray-600 bg-gray-300 px-4 py-2 rounded
                        ${currentPage === page ? "bg-gray-600 text-white" : "hover:bg-gray-500 hover:text-white"} 
                        ${page === '...' ? 'cursor-default' : ''}`}
                    disabled={page === '...'}
                >
                    {page === '...' ? '...' : page}
                </button>
            ))}

            {/* Siguiente */}
            <button
                onClick={handleNextPage}
                disabled={!hasNextPage}
                className={`bg-gray-600 text-white px-4 py-2 rounded
                  ${!hasNextPage ? "cursor-not-allowed" : "hover:bg-gray-500"}`}
            >
                <FaArrowRight />
            </button>
        </div>
    );
};
