window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

// Row selection and sum calculation for projections table
window.projectionsRowSelection = {
    selectedRows: new Set(),
    longPressTimer: null,
    longPressThreshold: 500, // milliseconds
    touchStartPos: null,
    moveThreshold: 10, // pixels
    isScrolling: false,
    longPressTriggered: false,
    
    initialize: function() {
        const table = document.getElementById('projections');
        if (!table) return;
        
        const tbody = table.querySelector('tbody');
        if (!tbody) return;
        
        // Clear any existing listeners
        this.cleanup();
        
        // Add click listener to tbody
        tbody.addEventListener('click', this.handleRowClick.bind(this), true);
        
        // Add touch event listeners for touch devices
        tbody.addEventListener('touchstart', this.handleTouchStart.bind(this), true);
        tbody.addEventListener('touchmove', this.handleTouchMove.bind(this), true);
        tbody.addEventListener('touchend', this.handleTouchEnd.bind(this), true);
        tbody.addEventListener('touchcancel', this.handleTouchCancel.bind(this), true);
        
        // Prevent context menu on long press
        tbody.addEventListener('contextmenu', this.handleContextMenu.bind(this), true);
        
        // Add document click listener to deselect when clicking outside
        document.addEventListener('click', this.handleDocumentClick.bind(this), true);
        document.addEventListener('touchend', this.handleDocumentTouch.bind(this), true);
        
        // Create popup if it doesn't exist
        if (!document.getElementById('selection-sum-popup')) {
            this.createPopup();
        }
    },
    
    handleRowClick: function(event) {
        // Check if clicked element or its parents contain specific classes that should trigger other actions
        let target = event.target;
        while (target && target.tagName !== 'TR') {
            // Don't interfere with these interactive elements
            if (target.classList.contains('materialize-history') || 
                target.classList.contains('edit-adjustment')) {
                return;
            }
            target = target.parentElement;
        }
        
        if (!target || target.tagName !== 'TR') return;
        
        // On touch devices, ignore click events for row selection (use long press instead)
        // Check if this is a touch-generated click event
        if (event.pointerType === 'touch' || 
            (event.sourceCapabilities && event.sourceCapabilities.firesTouchEvents)) {
            // Only handle Ctrl+click on non-touch devices, so let touch clicks pass through for editing
            return;
        }
        
        // Check if Ctrl key is pressed for multi-selection (desktop only)
        if (event.ctrlKey || event.metaKey) {
            event.stopPropagation();
            event.preventDefault();
            
            const rowId = this.getRowId(target);
            
            if (target.classList.contains('selected-row')) {
                target.classList.remove('selected-row');
                this.selectedRows.delete(rowId);
            } else {
                target.classList.add('selected-row');
                this.selectedRows.add(rowId);
            }
            
            this.updateSumPopup();
        }
    },
    
    handleTouchStart: function(event) {
        let target = event.target;
        while (target && target.tagName !== 'TR') {
            // Don't interfere with these interactive elements
            if (target.classList.contains('materialize-history') || 
                target.classList.contains('edit-adjustment')) {
                return;
            }
            target = target.parentElement;
        }
        
        if (!target || target.tagName !== 'TR') return;
        
        // Store touch start position to detect movement
        const touch = event.touches[0];
        this.touchStartPos = { x: touch.clientX, y: touch.clientY };
        this.isScrolling = false;
        this.longPressTriggered = false;
        
        // Start long press timer
        this.longPressTimer = setTimeout(() => {
            if (!this.isScrolling) {
                this.longPressTriggered = true;
                this.handleLongPress(target, event);
            }
        }, this.longPressThreshold);
    },
    
    handleTouchMove: function(event) {
        if (!this.touchStartPos) return;
        
        const touch = event.touches[0];
        const deltaX = Math.abs(touch.clientX - this.touchStartPos.x);
        const deltaY = Math.abs(touch.clientY - this.touchStartPos.y);
        
        // If moved too much, cancel long press and mark as scrolling
        if (deltaX > this.moveThreshold || deltaY > this.moveThreshold) {
            this.isScrolling = true;
            this.cancelLongPress();
        }
    },
    
    handleTouchEnd: function(event) {
        // If long press was triggered, prevent the click event from firing
        if (this.longPressTriggered) {
            event.preventDefault();
            event.stopPropagation();
            
            // Provide haptic feedback only when long press completes successfully
            // This ensures vibration happens only after the user lifts their finger
            // and not during scrolling
            if (navigator.vibrate) {
                navigator.vibrate(50);
            }
        }
        
        this.cancelLongPress();
        this.touchStartPos = null;
        this.isScrolling = false;
        
        // Reset the flag after a short delay to handle event propagation
        setTimeout(() => {
            this.longPressTriggered = false;
        }, 100);
    },
    
    handleTouchCancel: function(event) {
        this.cancelLongPress();
        this.touchStartPos = null;
        this.isScrolling = false;
    },
    
    handleLongPress: function(target, event) {
        // Prevent the default touch behavior (including triggering click events)
        if (event && event.preventDefault) {
            event.preventDefault();
        }
        
        // Note: Haptic feedback is now triggered in handleTouchEnd when long press completes.
        // This improves UX by providing feedback only after the user lifts their finger,
        // ensuring no vibration occurs during scrolling or if the gesture is cancelled.
        
        const rowId = this.getRowId(target);
        
        if (target.classList.contains('selected-row')) {
            target.classList.remove('selected-row');
            this.selectedRows.delete(rowId);
        } else {
            target.classList.add('selected-row');
            this.selectedRows.add(rowId);
        }
        
        this.updateSumPopup();
    },
    
    cancelLongPress: function() {
        if (this.longPressTimer) {
            clearTimeout(this.longPressTimer);
            this.longPressTimer = null;
        }
    },
    
    handleContextMenu: function(event) {
        // Prevent context menu from appearing on table rows
        // This prevents the native context menu that can appear on long press
        let target = event.target;
        let depth = 0;
        const maxDepth = 10; // Safety limit to prevent infinite loops
        
        while (target && target.tagName !== 'TR' && depth < maxDepth) {
            target = target.parentElement;
            depth++;
        }
        
        if (target && target.tagName === 'TR') {
            event.preventDefault();
            return false;
        }
    },
    
    handleDocumentClick: function(event) {
        const table = document.getElementById('projections');
        const popup = document.getElementById('selection-sum-popup');
        
        // Check if click is outside the table and popup
        if (table && !table.contains(event.target) && 
            popup && !popup.contains(event.target)) {
            this.clearSelection();
        }
    },
    
    handleDocumentTouch: function(event) {
        const table = document.getElementById('projections');
        const popup = document.getElementById('selection-sum-popup');
        const target = event.target;
        
        // Check if touch is outside the table and popup
        if (table && !table.contains(target) && 
            popup && !popup.contains(target)) {
            this.clearSelection();
        }
    },
    
    getRowId: function(row) {
        // Use row index as unique identifier
        return Array.from(row.parentElement.children).indexOf(row);
    },
    
    getAmountFromRow: function(row) {
        // Amount is in the 3rd column (index 2)
        const amountCell = row.cells[2];
        if (!amountCell) return 0;
        
        const amountText = amountCell.textContent.trim();
       
        const amount = parseFloat(amountText);
        return isNaN(amount) ? 0 : amount;
    },
    
    updateSumPopup: function() {
        const popup = document.getElementById('selection-sum-popup');
        if (!popup) return;
        
        if (this.selectedRows.size === 0) {
            popup.classList.remove('show');
            return;
        }
        
        // Calculate sum
        const table = document.getElementById('projections');
        const rows = table.querySelectorAll('tbody tr');
        let sum = 0;
        
        rows.forEach((row, index) => {
            if (this.selectedRows.has(index)) {
                sum += this.getAmountFromRow(row);
            }
        });
        
        // Update popup content
        const sumElement = popup.querySelector('.sum-value');
        const countElement = popup.querySelector('.row-count');
        
        sumElement.textContent = sum.toLocaleString('en-US', { 
            minimumFractionDigits: 2, 
            maximumFractionDigits: 2 
        });
        countElement.textContent = this.selectedRows.size;
        
        popup.classList.add('show');
    },
    
    createPopup: function() {
        const popup = document.createElement('div');
        popup.id = 'selection-sum-popup';
        popup.className = 'selection-sum-popup';
        popup.innerHTML = `
            <div class="popup-content">
                <div class="popup-header">
                    <span><span class="row-count">0</span> row(s) selected</span>
                    <button class="close-btn" onclick="projectionsRowSelection.clearSelection()">×</button>
                </div>
                <div class="popup-body">
                    <strong>Total Amount:</strong> <span class="sum-value">0.00</span>
                </div>
            </div>
        `;
        document.body.appendChild(popup);
    },
    
    clearSelection: function() {
        const table = document.getElementById('projections');
        if (table) {
            table.querySelectorAll('tr.selected-row').forEach(row => {
                row.classList.remove('selected-row');
            });
        }
        this.selectedRows.clear();
        this.updateSumPopup();
    },
    
    cleanup: function() {
        this.cancelLongPress();
        this.selectedRows.clear();
        this.touchStartPos = null;
        this.isScrolling = false;
        this.longPressTriggered = false;
        
        // Remove document event listeners
        document.removeEventListener('click', this.handleDocumentClick.bind(this), true);
        document.removeEventListener('touchend', this.handleDocumentTouch.bind(this), true);
        
        const popup = document.getElementById('selection-sum-popup');
        if (popup) {
            popup.classList.remove('show');
        }
    }
};