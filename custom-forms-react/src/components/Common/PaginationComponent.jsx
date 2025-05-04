import Pagination from 'react-bootstrap/Pagination';

const PaginationComponent = ({ currentPage, totalPages, onPageChange }) => {
    if (totalPages <= 1) return null;

    const handlePageClick = (pageNumber) => {
        if (pageNumber !== currentPage) {
            onPageChange(pageNumber);
        }
    };

    let items = [];
    const maxPagesToShow = 5; 
    let startPage, endPage;

    if (totalPages <= maxPagesToShow + 2) { 
        startPage = 1;
        endPage = totalPages;
    } else {
        if (currentPage <= Math.ceil(maxPagesToShow / 2)) {
            startPage = 1;
            endPage = maxPagesToShow;
        } else if (currentPage + Math.floor(maxPagesToShow / 2) >= totalPages) {
            startPage = totalPages - maxPagesToShow + 1;
            endPage = totalPages;
        } else {
            startPage = currentPage - Math.floor(maxPagesToShow / 2);
            endPage = currentPage + Math.floor(maxPagesToShow / 2);
        }
    }

    items.push(
        <Pagination.First key="first" onClick={() => handlePageClick(1)} disabled={currentPage === 1} />
    );
    items.push(
        <Pagination.Prev key="prev" onClick={() => handlePageClick(currentPage - 1)} disabled={currentPage === 1} />
    );

    if (startPage > 1) {
        items.push(<Pagination.Ellipsis key="start-ellipsis" disabled />);
    }

    for (let number = startPage; number <= endPage; number++) {
        items.push(
            <Pagination.Item key={number} active={number === currentPage} onClick={() => handlePageClick(number)}>
                {number}
            </Pagination.Item>,
        );
    }

    if (endPage < totalPages) {
        items.push(<Pagination.Ellipsis key="end-ellipsis" disabled />);
    }

    items.push(
        <Pagination.Next key="next" onClick={() => handlePageClick(currentPage + 1)} disabled={currentPage === totalPages} />
    );

    items.push(
        <Pagination.Last key="last" onClick={() => handlePageClick(totalPages)} disabled={currentPage === totalPages} />
    );


    return (
        <Pagination className="justify-content-center mt-4">{items}</Pagination>
    );
};

export default PaginationComponent;