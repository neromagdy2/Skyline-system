// FAQ Bubble Component
document.addEventListener('DOMContentLoaded', function () {
    // Create FAQ bubble
    const faqBubble = document.createElement('div');
    faqBubble.id = 'faq-bubble';
    faqBubble.innerHTML = `
        <div class="faq-bubble">
            <div class="faq-icon">?</div>
        </div>
    `;

    // Create FAQ overlay
    const faqOverlay = document.createElement('div');
    faqOverlay.id = 'faq-overlay';
    faqOverlay.innerHTML = `
        <div class="faq-modal">
            <div class="faq-header">
                <h3>Frequently Asked Questions</h3>
                <button class="faq-close">&times;</button>
            </div>
            <div class="faq-content">
                <div class="faq-item">
                    <div class="faq-question">
                        <span>How do I book a flight?</span>
                        <span class="faq-toggle">+</span>
                    </div>
                    <div class="faq-answer">
                        <ol>
                            <li>Search for flights using the search form on the homepage</li>
                            <li>Select your preferred flight from the results</li>
                            <li>Choose your seat class and specific seats</li>
                            <li>Click "Reserve seats" to proceed to payment</li>
                            <li>Complete payment using our secure Stripe payment system</li>
                        </ol>
                    </div>
                </div>
                
                
                <div class="faq-item">
                    <div class="faq-question">
                        <span>How do I search for flights?</span>
                        <span class="faq-toggle">+</span>
                    </div>
                    <div class="faq-answer">
                        <p>Use the search form on the homepage:</p>
                        <ul>
                            <li>Select departure and arrival airports</li>
                            <li>Choose your travel date</li>
                            <li>Click "Search Flights"</li>
                            <li>Filter results by price, duration, or departure time</li>
                        </ul>
                    </div>
                </div>
                
                <div class="faq-item">
                    <div class="faq-question">
                        <span>What payment methods are accepted?</span>
                        <span class="faq-toggle">+</span>
                    </div>
                    <div class="faq-answer">
                        <p>We accept all major credit and debit cards through our secure Stripe payment system. Your payment information is encrypted and secure.</p>
                    </div>
                </div>
                <div class="faq-item">
                    <div class="faq-question">
                        <span>How to Download My ticket?</span>
                        <span class="faq-toggle">+</span>
                    </div>
                    <div class="faq-answer">
                        <p>Either Check you Email OR go into the bookings tab and press "Download Ticket"</p>
                    </div>
                </div>
                
                <div class="faq-item">
                    <div class="faq-question">
                        <span>Can I change my seat after booking?</span>
                        <span class="faq-toggle">+</span>
                    </div>
                    <div class="faq-answer">
                        <p>Seat changes are allowed before payment completion. Once payment is confirmed, seats are final. If you need different seats, you may need to cancel and rebook.</p>
                    </div>
                </div>
                
                <div class="faq-item">
                    <div class="faq-question">
                        <span>What are the different seat classes?</span>
                        <span class="faq-toggle">+</span>
                    </div>
                    <div class="faq-answer">
                        <ul>
                            <li><strong>Economy:</strong> Standard seating with basic amenities</li>
                            <li><strong>Business:</strong> Enhanced comfort with extra legroom</li>
                            <li><strong>First Class:</strong> Premium seating with maximum comfort and service</li>
                        </ul>
                    </div>
                </div>
                
                <div class="faq-item">
                    <div class="faq-question">
                        <span>How early should I arrive at the airport?</span>
                        <span class="faq-toggle">+</span>
                    </div>
                    <div class="faq-answer">
                        <p>We recommend arriving:</p>
                        <ul>
                            <li>2 hours before domestic flights</li>
                            <li>3 hours before international flights</li>
                        </ul>
                    </div>
                </div>
                
                <div class="faq-item">
                    <div class="faq-question">
                        <span>What if my flight is delayed or cancelled?</span>
                        <span class="faq-toggle">+</span>
                    </div>
                    <div class="faq-answer">
                        <p>In case of delays or cancellations, you will be notified via email and can choose to:</p>
                        <ul>
                            <li>Rebook on the next available flight</li>
                            <li>Receive a full refund</li>
                            <li>Choose alternative travel dates</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Add CSS styles
    const styles = `
        #faq-bubble {
            position: fixed;
            bottom: 20px;
            right: 20px;
            z-index: 1000;
        }
        
        .faq-bubble {
            width: 60px;
            height: 60px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            box-shadow: 0 4px 20px rgba(102, 126, 234, 0.4);
            transition: all 0.3s ease;
            animation: pulse 2s infinite;
        }
        
        .faq-bubble:hover {
            transform: scale(1.1);
            box-shadow: 0 6px 30px rgba(102, 126, 234, 0.6);
        }
        
        .faq-icon {
            color: white;
            font-size: 24px;
            font-weight: bold;
            font-family: Arial, sans-serif;
        }
        
        @keyframes pulse {
            0% { box-shadow: 0 4px 20px rgba(102, 126, 234, 0.4); }
            50% { box-shadow: 0 4px 30px rgba(102, 126, 234, 0.6); }
            100% { box-shadow: 0 4px 20px rgba(102, 126, 234, 0.4); }
        }
        
        #faq-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            z-index: 2000;
            display: none;
            align-items: center;
            justify-content: center;
            backdrop-filter: blur(5px);
        }
        
        #faq-overlay.active {
            display: flex;
        }
        
        .faq-modal {
            background: white;
            border-radius: 20px;
            max-width: 600px;
            max-height: 80vh;
            width: 90%;
            overflow: hidden;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
            animation: slideIn 0.3s ease-out;
        }
        
        @keyframes slideIn {
            from {
                opacity: 0;
                transform: translateY(-50px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        .faq-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .faq-header h3 {
            margin: 0;
            font-size: 24px;
        }
        
        .faq-close {
            background: none;
            border: none;
            color: white;
            font-size: 30px;
            cursor: pointer;
            padding: 0;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: background 0.3s ease;
        }
        
        .faq-close:hover {
            background: rgba(255, 255, 255, 0.2);
        }
        
        .faq-content {
            padding: 20px;
            max-height: calc(80vh - 80px);
            overflow-y: auto;
        }
        
        .faq-item {
            margin-bottom: 15px;
            border: 1px solid #e0e0e0;
            border-radius: 10px;
            overflow: hidden;
        }
        
        .faq-question {
            background: #f8f9fa;
            padding: 15px;
            cursor: pointer;
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-weight: 600;
            color: #333;
            transition: background 0.3s ease;
        }
        
        .faq-question:hover {
            background: #e9ecef;
        }
        
        .faq-toggle {
            font-size: 20px;
            font-weight: bold;
            color: #667eea;
            transition: transform 0.3s ease;
        }
        
        .faq-item.active .faq-toggle {
            transform: rotate(45deg);
        }
        
        .faq-answer {
            padding: 0 15px;
            max-height: 0;
            overflow: hidden;
            transition: all 0.3s ease;
            background: white;
        }
        
        .faq-item.active .faq-answer {
            padding: 15px;
            max-height: 500px;
        }
        
        .faq-answer p, .faq-answer ul, .faq-answer ol {
            margin: 0 0 10px 0;
            color: #666;
            line-height: 1.6;
        }
        
        .faq-answer ul, .faq-answer ol {
            margin-left: 20px;
        }
        
        .faq-answer li {
            margin-bottom: 5px;
        }
        
        /* Responsive design */
        @media (max-width: 768px) {
            .faq-modal {
                width: 95%;
                max-height: 90vh;
            }
            
            .faq-header {
                padding: 15px;
            }
            
            .faq-header h3 {
                font-size: 20px;
            }
            
            .faq-content {
                padding: 15px;
            }
            
            .faq-question {
                padding: 12px 15px;
                font-size: 14px;
            }
            
            .faq-answer {
                font-size: 14px;
            }
        }
    `;
    
    // Add styles to head
    const styleSheet = document.createElement('style');
    styleSheet.textContent = styles;
    document.head.appendChild(styleSheet);
    
    // Add elements to body
    document.body.appendChild(faqBubble);
    document.body.appendChild(faqOverlay);
    
    // Event listeners
    faqBubble.addEventListener('click', function() {
        faqOverlay.classList.add('active');
        document.body.style.overflow = 'hidden';
    });
    
    faqOverlay.addEventListener('click', function(e) {
        if (e.target === faqOverlay) {
            closeFAQ();
        }
    });
    
    document.querySelector('.faq-close').addEventListener('click', closeFAQ);
    
    function closeFAQ() {
        faqOverlay.classList.remove('active');
        document.body.style.overflow = '';
    }
    
    // FAQ item toggle
    document.querySelectorAll('.faq-question').forEach(question => {
        question.addEventListener('click', function() {
            const item = this.parentElement;
            const isActive = item.classList.contains('active');
            
            // Close all other items
            document.querySelectorAll('.faq-item').forEach(otherItem => {
                otherItem.classList.remove('active');
            });
            
            // Toggle current item
            if (!isActive) {
                item.classList.add('active');
            }
        });
    });
});
